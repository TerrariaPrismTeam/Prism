#!/usr/bin/env bash

mkdir -p bin
mcs -out:bin/ExtractReLogic.exe -target:exe Program.cs

