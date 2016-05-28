#!/usr/bin/env bash

command -v mono >/dev/null 2>&1 || { echo >&2 "You must install Mono to use Prism. See the documentation."; exit 1;  }

if [ "$(uname)" == "Darwin" ]; then
	LD_LIBRARY_PATH=$(pwd)/osx:$LD_LIBRARY_PATH
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
	LD_LIBRARY_PATH=$(pwd)/lib64:$LD_LIBRARY_PATH
fi

mono Prism.exe
