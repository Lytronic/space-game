#!/usr/bin/env bash
set -euo pipefail

export VERSION="0.1.0"
export BUILDDIR="build"

function reset_builddir () {
  rm -rf ./$BUILDDIR
  mkdir $BUILDDIR
}


reset_builddir

godot-mono --export-release "Linux x86_64" "build/Microgravity.x86_64"
cd $BUILDDIR
tar cf "../Microgravity-$VERSION-x86_64-linux.tar.xz" *

cd ..
reset_builddir
godot-mono --export-release "Windows Desktop x86_64" "build/Microgravity.exe"
cd $BUILDDIR
zip "../Microgravity-$VERSION-x86_64-windows.zip" *


