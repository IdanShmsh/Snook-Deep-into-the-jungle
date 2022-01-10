As you can clearly see by the very first look at the game, the most central mechanic in the game is the Snake 
and its stretchy and wiggly mechanic.
And as I mentioned in the description of this repository, every single line of code was written by me. So when
one wants to create a snake that has soft-body physics, one will install a package and use it - very simple. 
Well, I didn't want to do that. In fact, the very start of this project was me creating these soft body physics
for fun, I got really excited that it worked, and I decided to make a game out of it.
So how did I create this soft-body physics? I did nothing new, it all has been done before and even better, but
as I previously mentioned this repository is me sharing my work and explaining it.
First, let's take a stretchy string and divide it into little units of mass. And between those, I will place tiny
springs, those will make this string stretchy. Now, how does a spring work? when it's stretched to be more than 
its rest length, it creates a pulling force - wishing to get back to its initial state. The same is true for when
it's squished, it creates a pushing force. Now this force it creates can be in some way described as being with a
linear relationship to the amount of difference in length from its rest length. For example, if a spring's length
is 10cm at rest and at 20cm it creates a certain force then at 30cm it will create a force twice as large.
So, I have a spring between every pair of masses, now for each spring, in each frame, I can calculate the force 
acting on the two masses based on the distance between them in comparison to the rest length I set the strings to 
be at.
Then, for each mass, I can sum up all the forces acting on it, produced by the springs it's attached to, and using 
Newtonian mechanics it's very easy to calculate the acceleration, velocity, and position of the mass at each frame. 
Here we go we now have a working code for a stretchy snake.
