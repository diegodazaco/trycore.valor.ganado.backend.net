from fastapi import FastAPI
from fastapi.responses import RedirectResponse
from models import ActivityInput, CalculationRequest, CalculationResponse, ActivityEvmResult, EvmIndicators
from calculator import calculate_activity_indicators, calculate_consolidated_indicators

app = FastAPI(
    title="EVM Calculator Service",
    description="Microservicio de cálculo de indicadores Earned Value Management (EVM)",
    version="1.0.0",
)


@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse(url="/docs")


@app.get("/health")
async def health_check():
    return {"status": "healthy"}


@app.post("/calculate", response_model=CalculationResponse)
async def calculate_evm(request: CalculationRequest) -> CalculationResponse:
    per_activity = [
        ActivityEvmResult(
            activity_id=activity.activity_id,
            name=activity.name,
            indicators=EvmIndicators(
                **calculate_activity_indicators(
                    bac=activity.budget_at_completion,
                    planned_percentage=activity.planned_progress_percentage,
                    actual_percentage=activity.actual_progress_percentage,
                    actual_cost=activity.actual_cost,
                )
            ),
        )
        for activity in request.activities
    ]

    consolidated_data = [
        {
            "bac": a.budget_at_completion,
            "planned_percentage": a.planned_progress_percentage,
            "actual_percentage": a.actual_progress_percentage,
            "actual_cost": a.actual_cost,
        }
        for a in request.activities
    ]

    return CalculationResponse(
        per_activity=per_activity,
        consolidated=EvmIndicators(**calculate_consolidated_indicators(consolidated_data)),
    )
