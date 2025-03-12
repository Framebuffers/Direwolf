This is a boilerplate I made for my projects using Nextjs, Prisma, Docker and Postgres. I had a hard time working out Dockerizing Nextjs and Prisma, so I made this to make it easier for me to start new projects. Hope that it helps you too!

Special thanks to Eli Front's [blog post](https://medium.com/@elifront/best-next-js-docker-compose-hot-reload-production-ready-docker-setup-28a9125ba1dc) for helping me figure out how to enable hot reloding, well his post and the github issue that i left commented.

It is also very important sharp needs to be lock to 0.32.6, or else it will give you a crazy amount of warning if you're using Nextjs optimized images.

To make things simpler I left two commands in the package.json, one for development and one for production. The development command will run the app in development mode, and the production command will build the app and run it in production mode.

If you want to run the app in development mode, you can run `npm run docker:compose:dev`.

For running on production mode you can run `npm run docker:compose:prod`, but know that you won't have hot reloading.

You also need to create your .env and .dockerignore files.

Good luck coding

------------------------------------------------------------------------------------------------------------------------------------------------------------------
This is a [Next.js](https://nextjs.org/) project bootstrapped with [`create-next-app`](https://github.com/vercel/next.js/tree/canary/packages/create-next-app).

## Getting Started

First, run the development server:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

You can start editing the page by modifying `app/page.tsx`. The page auto-updates as you edit the file.

This project uses [`next/font`](https://nextjs.org/docs/basic-features/font-optimization) to automatically optimize and load Inter, a custom Google Font.

## Learn More

To learn more about Next.js, take a look at the following resources:

- [Next.js Documentation](https://nextjs.org/docs) - learn about Next.js features and API.
- [Learn Next.js](https://nextjs.org/learn) - an interactive Next.js tutorial.

You can check out [the Next.js GitHub repository](https://github.com/vercel/next.js/) - your feedback and contributions are welcome!

## Deploy on Vercel

The easiest way to deploy your Next.js app is to use the [Vercel Platform](https://vercel.com/new?utm_medium=default-template&filter=next.js&utm_source=create-next-app&utm_campaign=create-next-app-readme) from the creators of Next.js.

Check out our [Next.js deployment documentation](https://nextjs.org/docs/deployment) for more details.
