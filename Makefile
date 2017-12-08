
CC = gcc
RUNTIME = mono --debug

all:
	cd external/World && make && $(CC) -shared -Wl,-soname,libWorld.so -o libWorld.so build/objs/world/*.o
	cd external/nclang && nuget restore && msbuild
	cd external/World && $(RUNTIME) ../../external/nclang/samples/PInvokeGenerator/bin/Debug/PInvokeGenerator.exe --out:../../world-sharp/world-sharp.cs --lib:World --ns:WorldSharp.Interop --match:world --arg:-x --arg:c++ --arg:-Isrc src/world/*.h
	cp external/World/libWorld.so .
	msbuild
