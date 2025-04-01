# 🐺 Direwolf

A data analysis framework for Autodesk Revit. Extracts, serialises and stores parameters from BIM models in fractions of a second.

This project requires [the direwolf-db container](https://github.com/Framebuffers/direwolf-db) to function. Please clone that repo and run the `docker-compose` file provided before using this file.


⚠️ **This is not a finalised product!** ⚠️
This is a proof-of-concept. It may be subject to change and implementation details may change in the future.

## ℹ️ What is in this repository?

Everything needed to connect to Revit, query, store and visualise parameters and insight from a BIM model. This includes:

- A proof of concept of Direwolf, separated in three main repositories:
        - `Direwolf`: The core implementation of the Direwolf framework. It is platform-agnostic.
        - `Direwolf.Revit`: The core implementation of DB-level Revit routines.
        - `Direwolf.Revit.UI`: Any code that implements `Autodesk.Revit.UI`.
- A whitepaper containing the theory behind Direwolf and its implementation.
- Two extra repositories:
        - `direwolf-db`: A PostgreSQL database, ready-to-go to receive Direwolf information.
        - `direwolf-dashboard`: A pre-made Grafana dashboard connected to `direwolf-db`, made to visualise data obtained from Direwolf.

## ⚠️ What isn't in this repository?

- A complete product ready for production.

## The Wolfpack

Imagine a pack of wolves going on a hunt. Each wolf knows what it wants, where to get it, and it’s waiting for a signal to start hunting. Whatever it catches, it goes back to its den.
This is basically all there is to Direwolf. Here’s a more in-depth look at how it works:
In many C-derived languages— C, C++, or in this case C#, there’s a data type called a `struct`: a type containing structured information. In C#, these are part of something called value types. Value types are pieces on information on themselves. Besides them, in C# there's also the reference type: basically all types of objects, like classes. These two make up the whole C# data model. 
However, there’s in C# a type called a `record`: which can be something in between. They're `value` types, meant to be immutable, they come with free JSON serialisation, and they hold structured information. On top of that, they can also act as a class, inheriting some of their behaviour.
Most importantly, the key difference between them is: value types are stored in the stack and reference types in the heap. 

### 📦 Prey

Prey types are read-only, structured records of small, ethereal pieces of information. They can be parameters, an HTTP request, a file— any kind of individual piece of information. They are held in the stack: just like a stack of plans on top of a desk. Prey types are held in RAM only as long as it’s being passed along the chain back to the den. Because they’re so small, there can be hundreds or thousands of them being created at once, ready to be processed as soon as they come.

### 🐺 Wolf

Wolf types are vessels that carry on a given task, executes them, and retrieve its result. They hold a reference (or a “callback”) to its summoner. They’re objects that only *execute:*  meaning that they **don’t hold** more than a cache of what’s working on right now. Therefore, if it fails to execute a task, it won’t bring the whole pack down with it.

Wolves store their catch inside a *stack*. As soon as the Wolf gets Prey back, it places it on top of a *stack* of information retrieved. When any kind of processing is ready, it pushes Prey **up to the den**. This is called a FIFO, or first-in first-out structure. 

This is called a *microservice*: small self-contained objects that have all the facilities to not only communicate with its summoner, but with other objects as well. This is done just by design: all wolves share the same interfaces (their DNA, or a *contract* as it’s technically called), and they all share a single summoner. If one goes down, only *its own request* fails. By logical conclusion, any other *dependent tasks* will fail by default— not like they’d be useful anyways.

### 💬 Howl

Howls are instructions for wolves to execute. They inherit from the same structure, and all of them share a single overridable function: `Execute()` . Any kind of data manipulation: connecting to an API, shaping data, handling requests; happens here. It has a callback to its Wolf, upon where any Prey is pushed up to the Wolf. 

This separation of concerns between processing and storage makes this architecture very malleable, atomic and modular by design. A howl can hold a very simple HTTP request that returns a Prey with just a 200 OK result, or a whole file. Only the Howl decides what the Wolf will push up the stack chain.

### 🔊 Howler

Howler is the *dispatch* of wolves. A Howler has three functions: queue up workers (our wolves), dispatch them (*decorating* them with their Howl and calling for its execution), and retrieve back their data (the Prey)  after the requests are complete, as they come along.

A Howler can contain several kinds of requests, from different places, all of them coming back to its den, where all the final processed information is stored.

### 🗄️ Wolfpack

Wolfpack is a collection of Howlers: a set of results from a given query. Wolfpack is the final value type of a Direwolf Query. It contains metadata from the hunt and the information gathered from it. This data type is then serialised and sent to a database, where it can be used for data science, machine learning, LLM-training— or anything.

### 🐺 Direwolf

Direwolf holds both the queries gathered, the connection to a database, and provisions to enqueue Howlers and send them hunting. However, apart from being the final destination of information and the dispatcher of dispatchers, it is **not by itself** a point of failure. If Direwolf goes down, each Howler will still hold the data of its own request— just nowhere to send it to. Each Howler’s den is made public by default, just in case this happens and some kind of independent method of dispatch and retrieval has to be made.

Each piece is independent by themselves, and they don’t necessarily depend on Direwolf to complete their task; just that it’s meant to be this way.

## 💻 Technology Stack

The technology stack is based primarily on .NET C#. However, it is built with modularity and independence by default.

### 🗃️ Direwolf.dll

All of the core functionality, regardless if it’s used in BIM or any other kind of application, is held inside a single library. It defines the c## ore behaviour and contracts that any object has to implement to work with Direwolf. The entire engine works only with interfaces, meaning that the core of Direwolf is completely platform-agnostic. Each platform can implement their own Direwolf by deriving from it, and just creating new interfaces that derive from these primitives.

### 🗃️ Direwolf.Revit.dll

Separate from the core engine, any kind of feature that **requires the implementation of RevitAPI.dll** is here. This library is designed to be able to be used for ACC Design Automation solutions because it does not inherit from any UI-dependent class.

Since Design Automation libraries need special kinds of commands, as well as implementing a completely different API; the aim is to keep this library *clean enough* to be able to be used in both desktop and cloud-based applications.

This library also implements a specific kind of RevitHowler and RevitHowl, where they all include  inside a reference to a Revit Document.

### 🗃️ Direwolf.Revit.UI.dll

Finally, this is the entry point for the Direwolf Revit add-in itself. This library implements any kind of UI `ExternalCommands`. It’s use is mostly limited to just implementing anything related to the UI API. 

### 🌐 direwolf-web

irewolf-web holds both the database and a dashboard example app to display information being retrieved from a model. It is built on top of:

- PostgreSQL for the database.
- Grafana for the dashboard.
- Prisma as an ORM for the dashboard.
- Docker to containerise the whole infrastructure.
        - Docker is an industry-standard tool in web services to easily containerise solutions into consistently reproducible virtual instances.
        - It separates the data from the application itself: each time a container spins up, it *creates* the application from scratch, but it reads the information it stored inside a volume.
        - This means that every single time a direwolf-web container is spun up, it will get a fresh copy of Postgres, Node.js and Prisma. It will perform the database migrations using Prisma, and shape a custom database schema tailor-made for this application.
        - Every single time, the database will be accessible in the same way and the dashboard will work the same way; as long as the Docker image is kept the same.

The whole web-side can be put on and offline with just a single `docker-compose up`. **It's not meant for production-use. It's just a demo.**

## 🏗️ Use cases

### 🏢 Model Health

The example app included performs the following tasks:

- Iterates through all the elements inside a Revit model.
- Checks for ~15 conditions known to impact the health of a Revit model.
- Generates a JSON file with the results.

These results are then sent to a Postgres DB. A web dashboard is waiting for any update on this database. When a new result is in, it reflects the last test results on screen.

### ⏳ Statistical analysis of load times

On large BIM projects, it is common for load times to grow exponentially over time— we just don’t know where or what are the main factors for this. We may infer that it’s larger family sizes, slower data connections, or a suboptimal model linking/workset topology. But it is possible to know this for a fact.

- When Revit loads, subscribe to an event to get notified when a document is being loaded.
- Subscribe to events related to any kind of load: new families, new links, opening files.
- Upon getting those notifications, start a stopwatch.
- Wait for the signal for them to be completed.
- Stop the stopwatch.
- Record the file, file version, date and time, event type and time taken for each workstation for each team member.

With this new information, now we know:

- Which is the heaviest model out of them all.
- Which one, regardless of size, *loads the slowest.*
- Is there any kind of error that’s making the load times slower?
- Over time, when the project gets more completed, when is the model getting considerably slower?
- Which are the main threats to *an specific project’s* health?

And, we could infer:

- Which are the better strategies for future BEP’s to adjust better to our needs.
- Which are the policies that hurt the performance the most.
- Which are items on our BEP’s that need improvement to keep projects on-track.
- and more!

### 🗣️ On-demand queries

If this information is then tied up with a LLM or search engine, custom queries could be translated from a prompt down to Direwolf queries, running the usual filters inside the Revit API. The API offers provisions for almost any kind of logical query imaginable, but it lacks a way to harness that power from an end-user’s perspective.

- A prompt can be translated into a logic operation.
- That logic operation can be translated into a Howl.
- That Howl can return a personalised request of information faster than writing a Schedule by hand.
- Using the API, create a new view/schedule with the results. Or, offer an export to Excel, or to a Database.
- Store that resulting prompt as feedback to a neural network, to better refine the FilteredElementCollector request from within the API, for it to then be shaped as a Direwolf Howl.

## 🔮 A framework for the future

Direwolf is not tied up to any specific application, API— or even use case. If in the future, a new BIM software solution appears and there’s an API to get data from, Direwolf can be quickly adapted to harness the value inside it. It is not just a Revit add-in. It’s bringing innovation to BIM, from the eyes of an architect and the skill of a software engineer.

This project is entirely made on my own, relying heavily on [Revit.Async](https://github.com/KennanChan/Revit.Async), an async implementation of the API. All checks model queries made for model health have been developed by myself (with a lot of Google and StackOverflow help). The source code is available at my GitHub repo. It is licenced under the Apache-2.0 terms and conditions.

### 🤝 Thanks

Thanks for all the people that have helped me get this far in this lonely journey. Special thanks to Luis for giving me the greatest privilege in the world of letting me learn to do this.

I’m open to any offer for employment— be it as a software engineer or inside BIM. If anyone’s interested, I’d like to get in touch!
