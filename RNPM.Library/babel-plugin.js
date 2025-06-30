module.exports = function (babel) {
  const { types: t } = babel;

  return {
    visitor: {
      FunctionDeclaration(path, state) {
        if (isReactComponent(path)) {
          extractSource(path, state);
        }
      },

      ArrowFunctionExpression(path, state) {
        if (isReactComponent(path, state)) {
          extractSource(path, state);
        }
      },

      ClassDeclaration(path, state) {
        if (isReactComponent(path)) {
          extractSource(path, state);
        }
      },
    },
  };

  function isReactComponent(path, state) {
    if (
      t.isFunctionDeclaration(path.node) ||
      t.isArrowFunctionExpression(path.node)
    ) {
      const parentPath = path.parentPath;

      if (
        parentPath &&
        (t.isExportDefaultDeclaration(parentPath.node) ||
          t.isExportNamedDeclaration(parentPath.node))
      ) {
        return true;
      }

      const body = path.node.body;
      let hasJSX = false;

      if (t.isBlockStatement(body)) {
        path.traverse({
          ReturnStatement(returnPath) {
            if (hasJSXElement(returnPath.node.argument)) {
              hasJSX = true;
            }
          },
        });
      } else if (hasJSXElement(body)) {
        hasJSX = true;
      }

      return hasJSX;
    }
s
    if (t.isClassDeclaration(path.node)) {
      const superClass = path.node.superClass;
      if (superClass) {
        if (
          (t.isMemberExpression(superClass) &&
            superClass.object.name === 'React' &&
            superClass.property.name === 'Component') ||
          (t.isIdentifier(superClass) && superClass.name === 'Component')
        ) {
          return true;
        }
      }

      let hasJSXInRender = false;
      path.traverse({
        ClassMethod(methodPath) {
          if (methodPath.node.key.name === 'render') {
            methodPath.traverse({
              ReturnStatement(returnPath) {
                if (hasJSXElement(returnPath.node.argument)) {
                  hasJSXInRender = true;
                }
              },
            });
          }
        },
      });

      return hasJSXInRender;
    }

    return false;
  }

  function hasJSXElement(node) {
    if (!node) return false;

    if (t.isJSXElement(node) || t.isJSXFragment(node)) {
      return true;
    }

    if (
      t.isCallExpression(node) &&
      (node.callee.name === 'createElement' ||
        (t.isMemberExpression(node.callee) &&
          node.callee.object.name === 'React' &&
          node.callee.property.name === 'createElement'))
    ) {
      return true;
    }

    return false;
  }

  function extractSource(path, state) {
    const { node, scope } = path;
    const { file } = state;

    let componentName;
    if (t.isFunctionDeclaration(node) || t.isClassDeclaration(node)) {
      componentName = node.id.name;
    } else if (t.isArrowFunctionExpression(node)) {
      const parentNode = path.parentPath.node;
      if (t.isVariableDeclarator(parentNode)) {
        componentName = parentNode.id.name;
      } else if (t.isExportDefaultDeclaration(parentNode)) {
        componentName = 'DefaultComponent';
      }
    }

    if (!componentName) {
      return;
    }

    const { start, end } = node;
    const sourceCode = file.code.slice(start, end);
    const filePath = state.filename || 'unknown';

    const program = path.scope.getProgramParent().path;
    program.unshiftContainer('body', [
      t.expressionStatement(
        t.assignmentExpression(
          '=',
          t.memberExpression(
            t.identifier('global'),
            t.identifier('__RNPM_registerComponentSource_queue')
          ),
          t.logicalExpression(
            '||',
            t.memberExpression(
              t.identifier('global'),
              t.identifier('__RNPM_registerComponentSource_queue')
            ),
            t.arrayExpression([])
          )
        )
      ),

      t.expressionStatement(
        t.assignmentExpression(
          '=',
          t.memberExpression(
            t.identifier('global'),
            t.identifier('__RNPM_registerComponentSource_safe')
          ),
          t.logicalExpression(
            '||',
            t.memberExpression(
              t.identifier('global'),
              t.identifier('__RNPM_registerComponentSource_safe')
            ),
            t.functionExpression(
              null,
              [
                t.identifier('name'),
                t.identifier('source'),
                t.identifier('path'),
              ],
              t.blockStatement([
                t.ifStatement(
                  t.binaryExpression(
                    '===',
                    t.unaryExpression(
                      'typeof',
                      t.memberExpression(
                        t.identifier('global'),
                        t.identifier('__RNPM_registerComponentSource')
                      )
                    ),
                    t.stringLiteral('function')
                  ),
                  t.blockStatement([
                    t.expressionStatement(
                      t.callExpression(
                        t.memberExpression(
                          t.identifier('global'),
                          t.identifier('__RNPM_registerComponentSource')
                        ),
                        [
                          t.identifier('name'),
                          t.identifier('source'),
                          t.identifier('path'),
                        ]
                      )
                    ),
                  ]),
                  t.blockStatement([
                    t.expressionStatement(
                      t.callExpression(
                        t.memberExpression(
                          t.memberExpression(
                            t.identifier('global'),
                            t.identifier('__RNPM_registerComponentSource_queue')
                          ),
                          t.identifier('push')
                        ),
                        [
                          t.objectExpression([
                            t.objectProperty(
                              t.identifier('name'),
                              t.identifier('name')
                            ),
                            t.objectProperty(
                              t.identifier('source'),
                              t.identifier('source')
                            ),
                            t.objectProperty(
                              t.identifier('path'),
                              t.identifier('path')
                            ),
                          ]),
                        ]
                      )
                    ),
                  ])
                ),
              ])
            )
          )
        )
      ),
    ]);

    // Use the safe registration function
    const registerCall = t.expressionStatement(
      t.callExpression(
        t.memberExpression(
          t.identifier('global'),
          t.identifier('__RNPM_registerComponentSource_safe')
        ),
        [
          t.stringLiteral(componentName),
          t.stringLiteral(sourceCode),
          t.stringLiteral(filePath),
        ]
      )
    );

    // Add the registration call
    program.pushContainer('body', registerCall);
  }
};
