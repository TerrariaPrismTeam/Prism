#!/usr/bin/env bash

command -v mono >/dev/null 2>&1 || { echo >&2 "You must install Mono to use Prism. See the documentation."; exit 1;  }

NLIBP=""

if [ "$(uname)" == "Darwin" ]; then
    NLIBP="osx"
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
    case "$(uname -m)" in
        "x86_64")
            NLIBP="lib64";;
        "x86"|"i386"|"i686")
            NLIBP="lib";;
        *)
            (>&2 echo "Your instruction set ($(uname -m)) isn't supported.")
            exit 1;;
    esac
else
    (>&2 echo "Your system ($(uname), $(uname -s)) isn't supported.")
fi

LD_LIBRARY_PATH="$(pwd)/${NLIBP}:$LD_LIBRARY_PATH" \
FNA_KEYBOARD_USE_SCANCODES=1 FNA_OPENGL_DISABLE_LATESWAPTEAR=1 FNA_OPENGL_FORCE_CORE_PROFILE=1 \
mono $MONO_OPTS Prism.exe "$@"

