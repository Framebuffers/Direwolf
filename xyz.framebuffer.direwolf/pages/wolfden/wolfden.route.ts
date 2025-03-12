import express, { Router } from "express";
import { wolfdenController } from "./wolfden.controller";
const router: Router = express.Router();

router.get("/", wolfdenController.getAllWolfdens);
router.post("/", wolfdenController.createWolfden);

export default router;