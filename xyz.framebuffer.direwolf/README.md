- The Direwolf Web Dashboard is built on top of Docker: a container-based virtualization system. If you're a beginner:
    - It allows you to _deploy_ this server just running a single command: `docker compose up -d`
    - You need [Docker Desktop](https://www.docker.com/products/docker-desktop/) to get going. If you're using Linux, you probably know what you're doing (you need both `docker` and `docker-compose` for this one).
    - If you use Windows, it will also ask if you want to use the **WSL 2** backend. Say yes. It will set it up for you.
        - Direwolf Web Dashboard works 100% inside the Linux layer of Docker. It is built to do so.

- The _stack_ (or technologies used to build this app) is:
    - [Next.js](https://nextjs.org/): a [React](https://react.dev/) framework for the frontend.
    
    - [Prisma](https://www.prisma.io/) to interface the web app with the PostgreSQL database.

    - [PostgreSQL](https://www.postgresql.org/) for the database layer.
        - All the data you generate is stored in here.
        - The credentials to access this database are stored inside the `.env` file. These are called **secrets**.
        - They should **never** be committed to the repository, therefore make sure they're **always** mentioned on the `.gitignore` file. It is set up by default.

    - [Npgsql](https://www.npgsql.org/) to interface Revit with the PostgreSQL database.

    - 