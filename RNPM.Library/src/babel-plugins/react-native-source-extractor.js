// babel-plugin-react-native-source-extractor.js
module.exports = function(babel) {
    const { types: t } = babel;
    
    return {
      visitor: {
        // Process function declarations
        FunctionDeclaration(path, state) {
          if (isReactComponent(path)) {
            extractSource(path, state);
          }
        },
        
        // Process arrow function expressions
        ArrowFunctionExpression(path, state) {
          if (isReactComponent(path, state)) {
            extractSource(path, state);
          }
        },
        
        // Process class declarations
        ClassDeclaration(path, state) {
          if (isReactComponent(path)) {
            extractSource(path, state);
          }
        }
      }
    };
  
    // Check if a node represents a React component
    function isReactComponent(path, state) {
      // For function declarations and arrow functions
      if (t.isFunctionDeclaration(path.node) || t.isArrowFunctionExpression(path.node)) {
        // Check the parent node for export declarations
        const parentPath = path.parentPath;
        
        // Check if it's a direct export
        if (parentPath && (
          t.isExportDefaultDeclaration(parentPath.node) || 
          t.isExportNamedDeclaration(parentPath.node)
        )) {
          return true;
        }
        
        // Check for JSX in the function body
        const body = path.node.body;
        let hasJSX = false;
        
        if (t.isBlockStatement(body)) {
          // For functions with block bodies, search for return statements with JSX
          path.traverse({
            ReturnStatement(returnPath) {
              if (hasJSXElement(returnPath.node.argument)) {
                hasJSX = true;
              }
            }
          });
        } else if (hasJSXElement(body)) {
          // For arrow functions with expression bodies
          hasJSX = true;
        }
        
        return hasJSX;
      }
      
      // For class declarations
      if (t.isClassDeclaration(path.node)) {
        // Check if it extends React.Component or Component
        const superClass = path.node.superClass;
        if (superClass) {
          if (
            (t.isMemberExpression(superClass) && 
             superClass.object.name === 'React' && 
             superClass.property.name === 'Component') ||
            (t.isIdentifier(superClass) && 
             superClass.name === 'Component')
          ) {
            return true;
          }
        }
        
        // Check for render method with JSX
        let hasJSXInRender = false;
        path.traverse({
          ClassMethod(methodPath) {
            if (methodPath.node.key.name === 'render') {
              methodPath.traverse({
                ReturnStatement(returnPath) {
                  if (hasJSXElement(returnPath.node.argument)) {
                    hasJSXInRender = true;
                  }
                }
              });
            }
          }
        });
        
        return hasJSXInRender;
      }
      
      return false;
    }
    
    // Check if a node is a JSX element
    function hasJSXElement(node) {
      if (!node) return false;
      
      if (t.isJSXElement(node) || t.isJSXFragment(node)) {
        return true;
      }
      
      if (t.isCallExpression(node) && 
          (node.callee.name === 'createElement' || 
           (t.isMemberExpression(node.callee) && 
            node.callee.object.name === 'React' && 
            node.callee.property.name === 'createElement'))) {
        return true;
      }
      
      return false;
    }
    
    // Extract source code and register it
    function extractSource(path, state) {
      const { node, scope } = path;
      const { file } = state;
      
      // Get component name
      let componentName;
      if (t.isFunctionDeclaration(node) || t.isClassDeclaration(node)) {
        componentName = node.id.name;
      } else if (t.isArrowFunctionExpression(node)) {
        // Try to get the variable name for arrow functions
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
      
      // Extract source code
      const { start, end } = node;
      const sourceCode = file.code.slice(start, end);
      const filePath = state.filename || 'unknown';
      
      // Create a call to register the source code
      const registerCall = t.expressionStatement(
        t.callExpression(
          t.memberExpression(
            t.identifier('global'),
            t.identifier('__RNPM_registerComponentSource')
          ),
          [
            t.stringLiteral(componentName),
            t.stringLiteral(sourceCode),
            t.stringLiteral(filePath)
          ]
        )
      );
      
      // Add global import if needed
      const program = path.scope.getProgramParent().path;
      
      // Add the registration call to the program
      program.pushContainer('body', registerCall);
    }
  };