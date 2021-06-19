# turret-defense
 A mobile game where the player defends hoards of zombies(?) against their central turret.

https://user-images.githubusercontent.com/44419210/122654928-713f1a00-d103-11eb-9e7f-9cf2abc5c1f4.mp4
 
# Special funcationality
 I always enjoy constructing my own systems for things; here are a couple examples of systems I created for this game:
 
 ### ***System for creating custom waves of enemies***
  This system allows you to easily select the method to call during the wave and what parameters to pass. I wanted to write a custom inspector to show what parameters the selected   method call takes, but I never got around to it.
  
![image](https://user-images.githubusercontent.com/44419210/122629085-35f30b80-d06f-11eb-9df4-e872bb17bbb5.png)

### ***A continuation of the modularity saga***
  In this project I also starting using some more event driven systems, highlighted in [this _fantastic_ Unite talk](https://youtu.be/raQ3iHhE_Kk).
  
### ***Mobile performance in mind***
  Since I wanted this game to run on mobile devices, I used a more ECS style approach for updating the enemies by having the enemies merely hold data and using the container for all the enemies to update them. This had a massive performance increase, going from enemies in the hundereds to enemies in the tens of thousands to introduce framerates less than 60! _Note: I'm not using Unity's ECS package in this project, but simply mimicing the functionality in this particular instance_
  
  ![image](https://user-images.githubusercontent.com/44419210/122629365-3db3af80-d071-11eb-8a1c-1eb5c47ba88f.png)
  
# Custom scripts
 All of the custom scripts that I wrote for this project are in 'Assets > Custom > Scripts'

