from typing import Optional
from pydantic import BaseModel, Field


class ActivityInput(BaseModel):
    activity_id: str
    name: str
    budget_at_completion: float = Field(gt=0)
    planned_progress_percentage: float = Field(ge=0, le=100)
    actual_progress_percentage: float = Field(ge=0, le=100)
    actual_cost: float = Field(ge=0)


class CalculationRequest(BaseModel):
    activities: list[ActivityInput]


class EvmIndicators(BaseModel):
    pv: float
    ev: float
    cv: float
    sv: float
    cpi: Optional[float]
    spi: Optional[float]
    eac: Optional[float]
    vac: Optional[float]
    cpi_interpretation: str
    spi_interpretation: str


class ActivityEvmResult(BaseModel):
    activity_id: str
    name: str
    indicators: EvmIndicators


class CalculationResponse(BaseModel):
    per_activity: list[ActivityEvmResult]
    consolidated: EvmIndicators
