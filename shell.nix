{ pkgs ? import <nixpkgs> { } }:

let
  runtimeLibs = with pkgs; [
    fontconfig
    freetype
    libx11
    libxext
    libxrender
    libxi
    libxrandr
    libxcursor
    libxfixes
    libxinerama
    libxcomposite
    libsm
    libice
    libxcb
    libglvnd
    mesa
    libgbm
    libdrm
  ];
in
pkgs.mkShell {
  buildInputs = runtimeLibs;

  shellHook = ''
    export LD_LIBRARY_PATH="${pkgs.lib.makeLibraryPath runtimeLibs}''${LD_LIBRARY_PATH:+:}$LD_LIBRARY_PATH"
  '';
}
