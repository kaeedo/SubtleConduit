import { defineConfig } from 'windicss/helpers'
const { transform } = require('windicss/helpers');

export default defineConfig ({
    preflight: {
        safelist:'div span ul li a h1 h2 h3 h4 h5 h6 aside section nav'
    },
    extract: {
        // accepts globs and file paths relative to project root
        include: [
            'index.html',
            './**/*.fs',
        ]
    },
    plugins: [transform('tailwindcss-textshadow')],
    theme: {
        extend: {
            colors:{
                'conduit-green':'#5CB85C',
                'conduit-green-500':'#449d44'
            },
            textColors:{
                'conduit-green':'#5CB85C',
                'conduit-green-500':'#449d44'
            }
        },
    }
})
