<img width="256" alt="dwolf-r2-512" class="center" src="https://github.com/user-attachments/assets/24707a0a-4212-4442-b4a3-33b61aade71e" />

# 🐺 Direwolf

Data extraction, manipulation and transformation framework for BIM applications.

An experimental data extraction engine, data transport protocol and MCP-based server for BIM applications. This implementation is built for Autodesk Revit 2025 and up.

⚠️ **This is not a finalised product!** ⚠️

This is a proof-of-concept. It may be subject to change and implementation details may change in the future. **I AM NOT RESPONSIBLE FOR ANY LOSS OF DATA, DAMAGED MODELS OR ANYTHING OF THE SORT.** 
Some features are in constant, fast development. They may be broken, they may break in the future, keep an eye out for new features and improvements!

## ✅ Changelog
- **v0.2-alpha:**
  - Complete rewrite of the engine,
  - implementation of a new data transport protocol (Wolfpack), a RAM-based Database (Wolfden) and server (Hunter).
  - Preliminary support for MCP.
  - Completely new object definitions.
  - Engine is completely independent of the running Revit application (just provide a Document and you're ready to go!),
  - and much more!
- **v0.1-alpha:**
  - Proof of concept.
  - Users have been experiencing problems setting it up. I am working right now for a patch to make it easier to get up and running, as well as a working binary.
  - If you want it to work right now, **make sure the DLL's from the `Direwolf.Revit.UI\Libraries` folder are in the same place as the rest of Direwolf's DLL's. The solution file + `.csproj` should do this automatically. I am aware of the issues and I'm working to sort them out.
  - Thanks for the feedback!


## ⚙️ Installation

- Decompress the ZIP file on `%appdata%\Autodesk\Revit\Addins\2025`
- Enjoy!

A message with the follow message will appear:

![image](https://github.com/user-attachments/assets/9792ba64-a3bd-4b13-849b-c111baca18ba)

Press `Always Load` (or `Load Once`), and it will load.

- Click on the `Add-In` tab.
- The buttons for each test will be on the Database tray.

![image](https://github.com/user-attachments/assets/cd049d3b-f029-4a70-adfb-73ab5ad667ae)

## ℹ️ What is in this repository?

Everything needed to connect to Revit, query, store and visualise parameters and insight from a BIM model. This includes:

- A proof of concept of Direwolf, spread across five (5) project folders:
  - `Direwolf`: Contains the main classes for Direwolf, Wolfden and Hunter.
  - `Direwolf.Definitions`: All the type and class definitions used across the framework.
  - `Direwolf.Definitions.Revit`: All Autodesk Revit-specific type and class definitions.
  - `Direwolf.Driver.MCP`: **[in development]** Model Context Protocol (MCP) driver for Direwolf.
  - `Direwolf.Revit`: Revit add-in.

## ⚠️ What is not in this repository?

- A complete product ready for production.

## 🤔 What is Direwolf?

**Direwolf** is a complete framework and protocol to move data between BIM software and MCP-compatible LLM (AI) agents. It has three (3) main components: `Direwolf` (a host), `Wolfden` (a database), `Hunter` (a server) and its data format: `Wolfpack` (a JSON-RCP 2.0-based data format).

It is built by a *BIM developer, for BIM developers,* to create a **common language** across all data sources and storage solutions-- no matter where they are, what they are, and for what it will be used. Be it moving *data inside an application*, from a model *out to another application* or to an *AI agent*, Direwolf proposes an efficient yet innovative solution for the technological challenges of today and tomorrow.

### ⚡ Innovations

- A completely RAM-based database engine.
- A very simple, low-level API approach: no unneccesary abstractions, no messy inheritance, stable and secure by design, built on just basic structs and lower-level operations.
- A REST/JSON-RCP 2.0 hybrid API.
- An MCP-compatible data format with BIM-specific features.
- An unique identification schema with extra features specific to BIM.
- An URI-based schema to access any resource, internal or external, on any BIM-related entity, property or taxonomy.
- and more things being cooked up right now!

## 🙋‍♂️ Want to help?

- Please send your requests, comments, suggestions as an issue in this repo.
- Want to contribute to the codebase? Feel free to fork and create a pull request!
- Share it with others!

# 💓 Thanks!

Thanks for everyone that has supported my journey so far. This is a passion project I've made for the Revit/BIM FOSS community, to help lower the barrier of entry to this field. I don't make anything out of this, other than getting a lot of invaluable knowledge. 

If you want to help me keep on developing new amazing ideas on the bleeding edge of our world, I would love to join your team and work together! 

🐺


