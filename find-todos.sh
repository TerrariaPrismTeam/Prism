#!/usr/bin/env bash

if ! command -v rg --version >/dev/null 2>&1; then
    find . -type f -name "*.c" -print0 | xargs -0 -P$(nproc) grep -C 2 -F -e "TODO" -e "FIXME"
else
    rg -n -C 2 -g "*.cs" -e "TODO" -e "FIXME" --heading --color always
fi

