import express from "express";
import wolfdenRouter from "./wolfden/wolfden.route";
const registerRoutes = (app: express.Application) => {
  app.use("/wolfden", wolfdenRouter);
};
export default registerRoutes;