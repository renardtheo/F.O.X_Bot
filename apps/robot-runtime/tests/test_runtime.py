from fastapi.testclient import TestClient
from fox_robot.app import app

client = TestClient(app)

def test_health():
    assert client.get("/health").json()["status"] == "ok"

def test_capabilities():
    assert "face.expression" in client.get("/capabilities").json()["capabilities"]

def test_invalid_expression():
    assert client.post("/expressions", json={"expression": "unknown"}).status_code == 400
