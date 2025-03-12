import express from "express";
import wolfdenRouter from "./wolfden/wolfden.route";
import preyRouter from "./prey/prey.route";
const registerRoutes = (app: express.Application) => {
  app.use("/prey", preyRouter);
  app.use("/wolfden", wolfdenRouter);
};
export default registerRoutes;