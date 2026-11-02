from dataclasses import dataclass
from typing import Protocol
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel


class RobotDriver(Protocol):
    def set_expression(self, expression: str) -> str: ...
    def capabilities(self) -> list[str]: ...


@dataclass
class SimulationDriver:
    expression: str = "idle"

    def set_expression(self, expression: str) -> str:
        if expression not in {"idle", "happy", "thinking", "curious", "alert", "sleeping"}:
            raise ValueError("Unsupported expression")
        self.expression = expression
        return expression

    def capabilities(self) -> list[str]:
        return ["face.expression", "face.blink"]


class ExpressionCommand(BaseModel):
    expression: str


driver: RobotDriver = SimulationDriver()
app = FastAPI(title="FOX Robot Runtime", version="0.1.0")


@app.get("/health")
def health():
    return {"status": "ok", "mode": "simulation"}


@app.get("/capabilities")
def capabilities():
    return {"capabilities": driver.capabilities()}


@app.post("/expressions")
def expression(command: ExpressionCommand):
    try:
        return {"expression": driver.set_expression(command.expression)}
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
