// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2022 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 * Modified by Intelligent Plant for use in App Store Connect adapter host app template.
 */

(() => {
    'use strict';

    const storedTheme = localStorage.getItem('theme');

    const getPreferredTheme = () => {
        if (storedTheme) {
            return storedTheme;
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    const setTheme = function (theme) {
        if (theme === 'auto' && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            document.documentElement.setAttribute('data-bs-theme', 'dark');
        } else {
            document.documentElement.setAttribute('data-bs-theme', theme);
        }

        const themeToggleButton = document.getElementById('theme-toggle');
        let themeIcon = themeToggleButton.querySelector('[data-theme-toggle-icon]');
        if (!themeIcon) {
            themeIcon = themeToggleButton.querySelector('i.fa-fw');
        }

        themeIcon.classList.remove('fa-sun', 'fa-moon', 'fa-circle-half-stroke');
        switch (theme) {
            case 'dark':
                themeIcon.classList.add('fa-moon');
                break;
            case 'auto':
                themeIcon.classList.add('fa-circle-half-stroke');
                break;
            case 'light':
            default:
                themeIcon.classList.add('fa-sun');
                break;
        }
    }

    setTheme(getPreferredTheme())

    const showActiveTheme = theme => {
        const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`);

        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active');
            element.querySelector('[data-active-indicator]').classList.add('d-none');
        });

        btnToActive.classList.add('active');
        btnToActive.querySelector('[data-active-indicator]').classList.remove('d-none')
    }

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        if (storedTheme !== 'light' || storedTheme !== 'dark') {
            setTheme(getPreferredTheme());
        }
    })

    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme());

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')
                    localStorage.setItem('theme', theme)
                    setTheme(theme)
                    showActiveTheme(theme)
                });
            });
    });
})();
