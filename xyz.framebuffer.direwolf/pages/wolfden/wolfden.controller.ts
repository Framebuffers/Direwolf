import { Request, Response } from "express";
import { wolfdenService } from "./wolfden.service";

class WolfdenController {
  constructor() {}

  async getAllWolfdens(req: Request, res: Response) {
    try {
      const result = await wolfdenService.getAllWolfdens();
      res.status(200).send(result);
    } catch (error) {
      console.error("Error fetching users:", error);
      res.status(500).send("Internal Server Error");
    }
  }

  async createWolfden(req: Request, res: Response) {
    try {
      const result = await wolfdenService.createWolfden(req.body);
      res.status(200).send(result);
    } catch (error) {
      console.error("Error fetching users:", error);
      res.status(500).send("Internal Server Error");
    }
  }
}

export const wolfdenController = new WolfdenController();