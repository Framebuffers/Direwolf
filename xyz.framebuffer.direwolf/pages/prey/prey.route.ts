import express, { Router } from "express";
import { preyController } from "./prey.controller";
const router: Router = express.Router();

router.get("/", preyController.getAllPrey);
router.post("/", preyController.createPrey);

export default router;