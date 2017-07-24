let path = require('path');
let UglifyJSPlugin = require('uglifyjs-webpack-plugin');
let webpack = require('webpack');

module.exports = {
  entry: {
    'pengine.core.web.3rdparty': './scripts/pengine.core.web.3rdparty.js',
    'pengine.core.web.main': './scripts/pengine.core.web.main.js'
  },
  devtool: 'source-map',
  output: {
    filename: '[name].min.js',
    path: path.resolve(__dirname, 'wwwroot/dist')
  },
  resolve: {
    alias: {
      'vue$': 'vue/dist/vue.esm.js'
    }
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: JSON.stringify('production')
      }
    }),
    new UglifyJSPlugin({
      compress: { warnings: false },
      sourceMap: true
    })
  ],
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /(node_modules)/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['env']
          }
        }
      }
    ]
  }
};
