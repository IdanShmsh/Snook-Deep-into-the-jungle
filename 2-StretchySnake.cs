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
Both the masses, called 'Joints', and the springs, called 'BodyParts' are c# classes I created to produce softbody
physics in any form. In my game I used these to create the snake.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class joint
{

    //parameters
    protected float mass; //the mass of the joint
    protected Vector3 position; //the position of the joint
    protected Vector3 velocity; //the velocity vector
    protected Vector3 sigmaF; //the force vector
    protected float resistance; //air resistance

    protected float criticalVelocity;
    protected float velocityTimeAdapterValue;

    public joint(float mass, Vector3 startPos, float resistance, float criticalVelocity) //create the joint
    {
        this.mass = mass;
        this.position = startPos;
        this.resistance = resistance;
        this.criticalVelocity = criticalVelocity;
        this.velocityTimeAdapterValue = (Mathf.Exp(1 / (2 * criticalVelocity * criticalVelocity))); //this is the function to get the adapter from the critical velocity
        //the critical velocity is whem the seccond derivative of velocityDeltaTime() function is 0

    }

    //return the location - for unity
    public Vector3 getPosition()
    {
        return this.position;
    }

    //update joint's state based on time between last frame
    public void updateState(float deltaTime)
    {

        if (deltaTime > 0.03f) deltaTime = 0.03f;
        if (velocity.magnitude >= 500) sigmaF = Vector3.zero;

        //update position - (physics)
        this.position += this.velocity * deltaTime;

        //update velocity based on force - (physics)
        this.velocity += (this.sigmaF / this.mass) * deltaTime;

        //update ve;ocity based on resistance - (physics)
        this.velocity -= ((this.velocity * this.resistance) / this.mass) * deltaTime;

        //reset force - force is being calculated every frame
        this.sigmaF = Vector3.zero;
    }

    protected float velocityDeltaTime(float velocity)
    {
        return (Mathf.Pow(this.velocityTimeAdapterValue, -1f * velocity * velocity));
    }

    //add force to joint
    public void addForce(Vector3 force)
    {
        this.sigmaF += force;
    }

    //set joint's position manually
    public void setPosition(Vector3 pos)
    {
        this.position = pos;
    }

    //get joint's velocity
    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    public void dumpVelocity()
    {
        this.velocity = Vector3.zero;
    }

    public void addVelocity(Vector3 velocity)
    {
        this.velocity = this.velocity + velocity;
    }
}
public class bodyPart
{
    joint j1; //joint pointers
    joint j2;

    Vector3 position; //the location of the body part
    Vector3 rotation; //the rotation of the body part
    Vector3 scale; //the scale of the body part

    float k; //the springiness constant
    float restL; //the rest length
    float dumping; //dumping index
    float limitLength; //lengthLimit

    public bodyPart(joint j1, joint j2, float k, float restL, float dumping) //create a body part
    {
        this.j1 = j1;
        this.j2 = j2;

        this.k = k;
        this.restL = restL;
        this.dumping = dumping;
        this.limitLength = -1;
    }
    public bodyPart(joint j1, joint j2, float k, float restL, float dumping, float limitLength) //create a body part
    {
        this.j1 = j1;
        this.j2 = j2;

        this.k = k;
        this.restL = restL;
        this.dumping = dumping;
        this.limitLength = limitLength;

        if (limitLength < restL) throw (new Exception("The length limit of a bodypart cannot be less than its rest length"));
    }

    //update body part state
    public void updateState()
    {
        float force; //the force that will be calculated the the body part generates


        //get the joint positions
        Vector3 pos1 = j1.getPosition(), pos2 = j2.getPosition();

        if (limitLength > 0)
        {
            if(VectorHandeler.distance(pos1, pos2) > limitLength)
            {
                j2.setPosition(pos1 + ((pos2 - pos1).normalized * limitLength));
            }
        }

        //the force is the product of the rest length and the distance between the two joints (the length prime / current) multiplied by the springiness constant
        force = (restL - Vector3.Distance(pos2, pos1)) * this.k;
        //add the dumping to the force, the dumping is the dot produt of the vector between the two joints normalized and the product of the two velocities, the dumping paramter is a multiplacation foctor
        force += Vector3.Dot(((pos2 - pos1) / Vector3.Distance(pos2, pos1)), (j2.getVelocity() - j1.getVelocity())) * dumping;

        j1.addForce((pos1 - pos2).normalized * (force));
        j2.addForce((pos2 - pos1).normalized * (force));
    }

    //for unity - get the transform parameters for the bodypart
    public (Vector3, Vector3, Vector3) getPRS()
    {
        //get the joint positions
        Vector3 pos1 = j1.getPosition(), pos2 = j2.getPosition();

        //(for unity objects)
        //the location of the bodypart will be in the middle cordinate between the two joints
        position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);
        //set the rotation to "look" at pos2 from pos1
        rotation = Quaternion.LookRotation(pos2 - pos1).eulerAngles;
        //scale the body part to fit the gap
        scale = new Vector3(1, Vector3.Distance(pos2, pos1) / 2, 1);

        return (this.position, this.rotation, this.scale);
    }
}

now that we have these classes built up, let's have a look on how I use them in my code.

All the following code parts are inside one class called Snake, which is a component of an empty object that contains all the
objects that are 'the snake itself'. The snake is made out of spheres that act as the joints and cylinders that act as the
BodyParts.

First, we build the snake. We create the spheres and cylinders that act as its body, with the addition of all the accessories
such as the hat or the rocket - those we don't care about right now.

private void buildSnake()
{
    //initiate the arrays
    jointObjs = new GameObject[length];
    bodyPartsObjs = new GameObject[length - 1];

    joints = new joint[length];
    bodyParts = new bodyPart[length - 1];

    //load the skin joint and body accourding to the name of the selected skin
    joint = Resources.Load<GameObject>(skinDirectories + "/" + skinName + "/Joint");
    body = Resources.Load<GameObject>(skinDirectories + "/" + skinName + "/Body");

    //load and instantiate the hat inside the head container
    hat = Instantiate(Resources.Load<GameObject>(hatDirectories + "/" + hatName + "/Hat"), headCont);

    //create the instances of the joints using the variables and create the joint objects 
    for (int i = 0; i < length; i++)
    {
        joints[i] = new joint(mass, new Vector3(0, 0, -i * restBodyLength), airResistance * (length - i), criticalVelocity);
        jointObjs[i] = Instantiate(joint, transform);
        jointObjs[i].transform.localPosition = joints[i].getPosition();
    }

    //the first joint is the tail and the last joint is the head
    joints[0] = tail = new headTail(mass, new Vector3(0, 0, 0), airResistance, thrust, criticalVelocity);
    joints[length - 1] = head = new headTail(mass, new Vector3(0, 0, -1 * (length - 1) * restBodyLength), airResistance, thrust, criticalVelocity);
    ((headTail)head).setHead(true); //tell the head that its the head

    //set the tail of each as the other
    ((headTail)head).setTail(tail);
    ((headTail)tail).setTail(head);

    //the first joint is the tail and the last joint is the head
    tailObj = jointObjs[0];
    headObj = jointObjs[length - 1];

    //set the position of the head container to the position of the head and the rotation to look at the joint before it
    headCont.localPosition = head.getPosition();
    headCont.localRotation = (head.Equals(joints[0]))
        ? VectorHandeler.lookAt(joints[1].getPosition(), joints[0].getPosition())
        : VectorHandeler.lookAt(joints[joints.Length - 2].getPosition(), joints[joints.Length - 1].getPosition());

    //create the instances of the bodyparts using the variables and create the bodypart objects 
    for (int i = 0; i < length - 1; i++)
    {
        bodyParts[i] = new bodyPart(joints[i], joints[i + 1], bodySpringiness, restBodyLength, 0);
        bodyParts[i].updateState();
        bodyPartsObjs[i] = Instantiate(body, transform);
        (Vector3 position, Vector3 rotation, Vector3 scale) = bodyParts[i].getPRS(); //get the 3d varibales of the bodypart and set the object as needed
        bodyPartsObjs[i].transform.localPosition = position;
        bodyPartsObjs[i].transform.localRotation = Quaternion.Euler(rotation + new Vector3(90, 0, 0));
        bodyPartsObjs[i].transform.localScale = scale;

        Instantiate(venomEffect, bodyPartsObjs[i].transform).SetActive(false); //create the venom effect as the bodyparts child and deactivate it
    }
}

Now the physics are not going to work by themselves, we only have the instances of the classes we created to calculate the way
each object acts, when being a part of a soft body. We now need to trigger these calculations, and also manually move the snake's
body objects to their culculated location.
Each frame (that we want to apply the physics to the snake in) we update the state of every joint and BodyPart, basically we tell
the BodyParts to calculate the forces they apply on the joints, and the joints to calculate their next position based on these forces
(as explained previously). After that we take the calculated positions of the joints and BodyParts, in addition to the calculated
rotation and scale of the BodyParts, and we transform the actual game objects to these 3D states.
What we did is basically took the mathematical model we created and made it visual.

private void updateSnakeState()
{
    //if the snake had just been revived anchor the snake in place (fixing the issue of the snake going crazy when reviving)
    if (reviveTime != -1)
    {
        reviveTime += Time.deltaTime;

        ((headTail)tail).setPosition(anchorPoint);
        jointObjs[0].transform.localPosition = tail.getPosition();
        for (int i = 1; i < length - 1; i++)
        {
            joints[i].setPosition(anchorPoint + new Vector3(0, 0, -i * restBodyLength));
            jointObjs[i].transform.localPosition = joints[i].getPosition();
            joints[i].dumpVelocity();
        }
        ((headTail)head).setPosition(anchorPoint + new Vector3(0, 0, -(length - 1) * restBodyLength)); ;
        jointObjs[length - 1].transform.localPosition = head.getPosition();

        if(reviveTime >= .5f)
        {
            reviveTime = -1;
        }
    }
    else
    {
        //set the position of the tail to the anchor point
        ((headTail)tail).setPosition(anchorPoint);
        jointObjs[0].transform.localPosition = tail.getPosition();
        for (int i = 1; i < length - 1; i++) //update the state of the joints (that are not the head or tail) and move the joint objects
        {
            joints[i].updateState(Time.deltaTime);
            jointObjs[i].transform.localPosition = joints[i].getPosition();

            if (rocket) //if the snake is rockest boosting - limit the distance each joint can be from the tail
            {
                if (tail.Equals(joints[0]))
                {
                    if (VectorHandeler.distance(joints[i].getPosition(), joints[0].getPosition()) > i * 1)
                    {
                        joints[i].setPosition(joints[0].getPosition() + ((joints[i].getPosition() - joints[0].getPosition()).normalized * i * 1));
                    }
                }
                else
                {
                    if (VectorHandeler.distance(joints[i].getPosition(), joints[joints.Length - 1].getPosition()) > (joints.Length - 1 - i) * 1)
                    {
                        joints[i].setPosition(joints[joints.Length - 1].getPosition() + ((joints[i].getPosition() - joints[joints.Length - 1].getPosition()).normalized * (joints.Length - 1 - i) * 1));
                    }
                }
            }
        }
        //eventually update the state of the head
        ((headTail)head).updateState(Time.deltaTime);
        jointObjs[length - 1].transform.localPosition = head.getPosition();
    }


    //update the state of the body parts set the objects acourdingly
    for (int i = 0; i < length - 1; i++)
    {
        bodyParts[i].updateState();
        (Vector3 position, Vector3 rotation, Vector3 scale) = bodyParts[i].getPRS();
        bodyPartsObjs[i].transform.localPosition = position;
        bodyPartsObjs[i].transform.localRotation = Quaternion.Euler(rotation + new Vector3(90, 0, 0));
        bodyPartsObjs[i].transform.localScale = new Vector3(bodyPartsObjs[i].transform.localScale.x, scale.y, bodyPartsObjs[i].transform.localScale.z);
    }

    //if the distance of any joint from the anchor point is greated than 100, move it back to the anchor point 
    for (int i = 1; i < length - 1; i++)
    {
        if (VectorHandeler.distance(joints[i].getPosition(), anchorPoint) >= 100)
        {
            joints[i].setPosition(anchorPoint + new Vector3(0, 0, -i * restBodyLength));
            joints[i].dumpVelocity();
        }
    }

    //the calculated score is the z position of the head divided by 3
    int tempScore = (int)(head.getPosition().z / 3);

    //if the calculated score is greater than the score of the snake - make the score the calculated score.
    if (tempScore > score)
    {
        score = tempScore;
    }

    //set the position and rotation of the head container
    headCont.localPosition = head.getPosition();
    headCont.localRotation = (head.Equals(joints[0]))
        ? VectorHandeler.lookAt(joints[1].getPosition(), joints[0].getPosition())
        : VectorHandeler.lookAt(joints[joints.Length - 2].getPosition(), joints[joints.Length - 1].getPosition());
}
