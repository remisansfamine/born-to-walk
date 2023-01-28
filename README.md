# Born to walk - **Unity/C#**
### ISART DIGITAL GP3, School Project: *Benjamin MARIN, Rémi GINER*  
<br>

<div style="text-align:center">

![HUContinuous](Annexes/HeadsUpContinuous.gif)

</div>

<!-- ABOUT THE PROJECT -->
# About The Project 
**Built with Unity 2021.3.5f1**

The goal of this project is to apply the principles of learning in a video game contexte. We decided to take on the challenge of developing an AI capable of learning to walk using a *ragdoll* as a physical representation.

# Table of contents
1. [Features](#features)
2. [Details](#details)
    - [Ragdoll Implementation](#ragdoll-implementation)
3. [In the future](#itf)
4. [Reference](#references)
5. [Versionning](#versionning)
6. [Autors](#authors)


# Features
- Genetic simulation that works with any type of Physical Controller
- Multilayer perceptron algorithms
- MLP and genetic population serialization
- Basic training for a ragdoll to stand up

# Details

## Ragdoll implementation
To integrate ragdoll physics into the project, we had to make that the MLP outputs supported by Unity's native ragdoll system. To do this, we had to research a bit how muscle joints work for a humanoid entity. After a bit of thought, we discovered that the only outputs our AI needed to give were different torque (scalar) values for each of the bones in the ragdoll.

<div style="text-align:center">

![MuscularJoints](Annexes/MuscularJoints.png)

*Muscular flexion scheme*
</div>


### **Character Joint Implementation**
By using the variables ``axis`` and ``wingAxis``, we were able to obtain a correct behavior of the joints. Using only 2 scalars allows us to better mimic the behavior of humanoid joints and limit the number of outputs. With 10 bones, we had 20 outputs in total.

```cs
    for (int i = 0; i < bones.Count - 1; ++i)
    {
        Bone currentBone = bones[i];

        float torque = outputs[i];
        float swingTorque =  outputs[i * 2];

        // axis: "The Direction of the axis around which the body is constrained."
        // swingAxis: "The secondary axis around which the joint can rotate."

        currentBon.rigidbody.AddRelativeTorque(torque * force * currentBon.characterJoint.axis, ForceMode.Impulse);
        currentBon.rigidbody.AddRelativeTorque(swingTorque * force * currentBon.characterJoint.swingAxis, ForceMode.Impulse);
    }
```

### **Collision Query and Ignore Collisions**
To see our population evolve properly, we had to instantiate each individual in the same position to check for evolution. Because of the dynamic instantiation, and because collisions between the bones of a ragdoll are very important, we had to ignore layers with other ragdolls on the fly. To do this, we had to ignore collisions between 26 of the 31 layers (because the first 5 layers are native to Unity) at the beginning of the program. Then, after creating our entire population, we had to recursively define the layers for each bone in each individual.

```cs
if (useLayerIgnore)
{
    for (int lai = layerOffset; lai < populationCount + layerOffset; lai++)
    {
        for (int lbi = lai + 1; lbi < populationCount + layerOffset; lbi++)
            Physics.IgnoreLayerCollision(lai, lbi, true);
    }

    SetLayers();
}

private void SetLayers()
{
    for (int i = 0; i < populationCount; i++)
        population[i].gameObject.SetLayerRecursively(layerOffset + i);
}

public static void SetLayerRecursively(this GameObject obj, int newLayer)
{
    obj.layer = newLayer;

    for (int i = 0; i < obj.transform.childCount; i++)
        obj.transform.GetChild(i).gameObject.SetLayerRecursively(newLayer);
}
```

<div style="text-align:center">

![Collisions](Annexes/Collisions.gif)

*Ragdoll training with collisions*
</div>

<div style="text-align:center">

![NoCollisions](Annexes/NoCollisions.gif)

*Ragdoll training without collisions*
</div>

## In the future:
In the future, we will try to strengthen the learning process by creating our own ragdolls to make them more accurate. Also, we will try to train them on our own game engine to have more accurate physics and faster algorithms using C++.


## References:
General references:
- https://www.youtube.com/watch?v=gn4nRCC9TwQ

Reinforcement learning:
- https://arxiv.org/pdf/2205.01906v2.pdf
- https://en.wikipedia.org/wiki/Q-learning

## Versionning
Git Lab for the versioning.

# Authors
* **Benjamin MARIN**
* **Rémi GINER**