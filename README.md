
fork from https://github.com/KhronosGroup/UnityGLTF

and fixed some bug for test ,version 2019-04-02

#build tool

####unity version 2017 .3.1 professional 

because use .net framework  4.0 ,unity  5.x can not compile ,free version can't   use  some feathers .

####visual studio 2017

build  for  gltf GLTFSerialization

##how to compile

open this project ,open  scene TestScene.unity open menu file/build setting ,switch webgl ,(may be you have to download webgl modul first),build setting ,increase memory  for webgl ,ex 512m ,then build ,

compile time may spend more than 10 minutes 

-----

#how to test?


before your test,checkout 

https://github.com/cx20/gltf-test



copy your build forder (../bin) into examples/unity ,

maybe like :

https://yourhost/examples/unity/index.html?model=BoxVertexColors&scale=1

then test .

----
#know issue

current UnityGltf not support some extra features  like 

gltfshader,uvtransform ,vertext color .

animation have bug .

memories  error, more than 256M,don't know how to fixed?

##about shader

there are no bug in editor,but not right show in webgl alone.


















