// eslint.config.js
import js from "@eslint/js";
import globals from "globals";
import reactHooks from "eslint-plugin-react-hooks";
import react from "eslint-plugin-react";

export default [
    {
        files: ["**/*.{js,jsx,ts,tsx}"],
        languageOptions: {
            ecmaVersion: "latest",
            sourceType: "module",
            globals: {
                ...globals.browser,
                ...globals.es2021,
            },
        },
        plugins: {
            react,
            "react-hooks": reactHooks,
        },
        rules: {
            ...js.configs.recommended.rules,
            ...react.configs.recommended.rules,

            // React recommended improvements
            "react/react-in-jsx-scope": "off", // React 17+ no longer requires import React
            "react/prop-types": "off",         // If using TypeScript or prefer not using PropTypes

            // Hooks best practices
            "react-hooks/rules-of-hooks": "error",
            "react-hooks/exhaustive-deps": "warn",

            // Code style defaults
            "semi": ["error", "always"],
            "quotes": ["error", "double"],
            "no-unused-vars": "warn",
            "no-console": "warn"
        },
        settings: {
            react: {
                version: "detect",
            },
        },
    },
];
