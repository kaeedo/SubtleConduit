const { transform } = require('windicss/helpers');

module.exports = {
    extract: {
        // accepts globs and file paths relative to project root
        include: [
            './index.html',
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
}
