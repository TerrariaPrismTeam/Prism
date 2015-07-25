#!/bin/bash

command -v mono >/dev/null 2>&1 || { echo >&2 "You must install Mono to use Prism. See the documentation."; exit 1;  }

LD_LIBRARY_PATH=$(pwd)/lib64:$LD_LIBRARY_PATH

mono Prism.exe
