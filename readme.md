This is my Unity project "MiniCB", a simple city builder [WIP] that I am currently developing to learn Unity DOTS. 
So far, I really like it, and it feels somewhat similar to our ecslite setup we used in Endzone 2. 
However, the API is quite extensive, and I'm still figuring out the best practices here and there. 
To get a better grasp of it, I’m creating this simple game.

The game is still in a very early stage.  
Currently, you can select the following buildings using the keyboard:  
- 1 - Gatherer (collects mushrooms)  
- 2 - Woodcutter (collects wood from trees)  
- 3 - Mason (collects stones)  

Use left-click to build — there are no restrictions yet.  
You’ll see some feedback in the console when selecting buildings and upon job completion.  

Camera movement is not implemented at this point.  
Everything is very basic.
No map generation, or spawning of settlers, yet.
There are going to be three different types of building:
- Entity To Resource-Converter - Find a specific resource entity in the buildings radius and collect its resource  
- Convert Input Resources to Output Resource - converts a defined amount of input-resources to a defined amout of output-resource **TO BE DONE**
- Spawn Entity - Spawn specific prefabs in the buildings radius **TO BE DONE**

**Key Elements:**

- **Buildings**  
  - Spawn jobs specific to their functionality  
  - Limit the number of active jobs according to their configured component  

- **Settlers**  
  - Execute available (free) jobs efficiently  
  - Can be setup to only exectue a subset of all job-types

- **Resources**  
  - Entities that represent collectable resources  
  - Can be harvested multiple times if configured accordingly in the `ResourceComponent`  


Jobs are used to manipulate the world. 
E.g. instructing a building-construction will result in a construction-job, or creating jobs to
move to a resource-entity and collect e.g. wood.



The job system will distribute jobs to settlers that are idle at the moment. 

## Key Features of the JobSystem 

**1. Job Life Cycle Management:**  
The system controls the entire lifecycle of a job – from starting, moving to the target, working on the task, finishing, and optionally returning to the owner.

**2. State-Based Processing:**  
Each job progresses through well-defined states (`MovingToTarget`, `ReachedTarget`, `Working`, `FinishedWorking`, `MovingToOwner`, etc.). The system checks and processes these states every update frame.

**3. Modular Job Logic:**  
The specific logic for different job types (e.g., construction, resource gathering) is implemented in separate classes via the `IJobLogic` interface, making the system flexible and extensible.

**4. Resource and Progress Management:**  
The system handles work progress, timers, and resource transfers (e.g., inventory management during resource collection), including updating a global inventory.

**5. Automatic Cleanup:**  
Upon job completion, all related references and components are cleaned up properly, and the worker (settler) is freed up for new tasks.


