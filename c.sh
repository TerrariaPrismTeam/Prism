#!/usr/bin/env bash

CONF="$@"

if [ -z "$CONF" ]; then
    CONF=DevBuild
fi

xbuild "/p:Configuration=$CONF" "Prism.sln"
