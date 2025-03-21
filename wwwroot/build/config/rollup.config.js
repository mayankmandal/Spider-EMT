'use strict'

const { babel } = require('@rollup/plugin-babel')

const pkg = require('../../package')
const year = new Date().getFullYear()
const banner = `/*!
 * SpiderWeb v${pkg.version} (${pkg.homepage})
 * Copyright 2014-${year} ${pkg.author}
 * Licensed under MIT (https://github.com/ColorlibHQ/SpiderEMT/blob/master/LICENSE)
 */`

module.exports = {
  input: 'build/js/SpiderEMT.js',
  output: {
    banner,
    file: 'dist/js/spideremt.js',
    format: 'umd',
    globals: {
      jquery: 'jQuery'
    },
    name: 'spideremt'
  },
  external: ['jquery'],
  plugins: [
    babel({
      exclude: 'node_modules/**',
      // Include the helpers in the bundle, at most one copy of each
      babelHelpers: 'bundled'
    })
  ]
}
