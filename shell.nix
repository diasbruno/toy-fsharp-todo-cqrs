{ nixpkgs ? import (fetchTarball("channel:nixos-23.11")) {} }:
nixpkgs.mkShell {
  name = "nfcqrs";
  packages = [(with nixpkgs.dotnetCorePackages; combinePackages [runtime_6_0 aspnetcore_6_0 sdk_8_0])];
  shellHook = ''
  export DOTNET_ROOT=${nixpkgs.dotnet-sdk_8};
  source $DOTNET_ROOT/nix-support/setup-hook;
  dotnet --info;
'';
}
