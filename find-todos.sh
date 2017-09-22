#!/usr/bin/env bash

if command rg >/dev/null 2>&1; then
    find . -type f -name "*.c" -print0 | xargs -0 -P$(nproc) grep -C 2 -F "TODO"
else
    rg -C 2 -g "*.cs" -e "TODO"
fi

