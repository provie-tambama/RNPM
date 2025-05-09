const path = require('path');
const { getConfig } = require('react-native-builder-bob/babel-config');
const pkg = require('../package.json');

// Path to the babel plugin in the library
const babelPluginPath = path.resolve(__dirname, '../src/babel-plugins/react-native-source-extractor.js');

const root = path.resolve(__dirname, '..');

module.exports = function (api) {
  api.cache(true);

  const baseConfig = getConfig(
    {
      presets: ['babel-preset-expo'],
      plugins: [babelPluginPath],
    },
    { root, pkg }
  );

  // Ensure plugins array exists and add our plugin
  if (!baseConfig.plugins) {
    baseConfig.plugins = [];
  }

  // Add the plugin if it's not already included
  if (!baseConfig.plugins.includes(babelPluginPath)) {
    baseConfig.plugins.push(babelPluginPath);
  }

  return baseConfig;
};
