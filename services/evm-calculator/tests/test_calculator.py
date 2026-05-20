import sys
import os
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

from calculator import (
    calculate_pv,
    calculate_ev,
    calculate_cv,
    calculate_sv,
    calculate_cpi,
    calculate_spi,
    calculate_eac,
    calculate_vac,
    interpret_cpi,
    interpret_spi,
    calculate_activity_indicators,
    calculate_consolidated_indicators,
)


class TestCalculatePv:
    def test_returns_planned_percentage_of_bac(self):
        assert calculate_pv(1000.0, 50.0) == 500.0

    def test_returns_zero_when_planned_percentage_is_zero(self):
        assert calculate_pv(1000.0, 0.0) == 0.0

    def test_returns_full_bac_when_planned_percentage_is_100(self):
        assert calculate_pv(1000.0, 100.0) == 1000.0


class TestCalculateEv:
    def test_returns_actual_percentage_of_bac(self):
        assert calculate_ev(1000.0, 40.0) == 400.0

    def test_returns_zero_when_actual_percentage_is_zero(self):
        assert calculate_ev(1000.0, 0.0) == 0.0


class TestCalculateCv:
    def test_positive_when_ev_greater_than_ac(self):
        assert calculate_cv(400.0, 300.0) == 100.0

    def test_negative_when_ev_less_than_ac(self):
        assert calculate_cv(400.0, 500.0) == -100.0

    def test_zero_when_ev_equals_ac(self):
        assert calculate_cv(400.0, 400.0) == 0.0


class TestCalculateSv:
    def test_positive_when_ev_greater_than_pv(self):
        assert calculate_sv(400.0, 300.0) == 100.0

    def test_negative_when_ev_less_than_pv(self):
        assert calculate_sv(300.0, 500.0) == -200.0


class TestCalculateCpi:
    def test_returns_ratio_of_ev_over_ac(self):
        result = calculate_cpi(400.0, 300.0)
        assert result == pytest.approx(1.3333, rel=1e-3)

    def test_returns_none_when_ac_is_zero(self):
        assert calculate_cpi(400.0, 0.0) is None

    def test_returns_one_when_ev_equals_ac(self):
        assert calculate_cpi(400.0, 400.0) == 1.0


class TestCalculateSpi:
    def test_returns_ratio_of_ev_over_pv(self):
        result = calculate_spi(400.0, 500.0)
        assert result == pytest.approx(0.8, rel=1e-3)

    def test_returns_none_when_pv_is_zero(self):
        assert calculate_spi(400.0, 0.0) is None

    def test_returns_one_when_ev_equals_pv(self):
        assert calculate_spi(400.0, 400.0) == 1.0


class TestCalculateEac:
    def test_returns_bac_divided_by_cpi(self):
        assert calculate_eac(1000.0, 1.25) == 800.0

    def test_returns_none_when_cpi_is_none(self):
        assert calculate_eac(1000.0, None) is None

    def test_returns_none_when_cpi_is_zero(self):
        assert calculate_eac(1000.0, 0.0) is None


class TestCalculateVac:
    def test_returns_bac_minus_eac(self):
        assert calculate_vac(1000.0, 800.0) == 200.0

    def test_negative_when_eac_exceeds_bac(self):
        assert calculate_vac(1000.0, 1200.0) == -200.0

    def test_returns_none_when_eac_is_none(self):
        assert calculate_vac(1000.0, None) is None


class TestInterpretCpi:
    def test_under_budget_when_cpi_greater_than_one(self):
        assert interpret_cpi(1.5) == "Bajo presupuesto"

    def test_over_budget_when_cpi_less_than_one(self):
        assert interpret_cpi(0.8) == "Sobre presupuesto"

    def test_on_budget_when_cpi_equals_one(self):
        assert interpret_cpi(1.0) == "En presupuesto"

    def test_message_when_cpi_is_none(self):
        result = interpret_cpi(None)
        assert result == "Sin costo real registrado"


class TestInterpretSpi:
    def test_ahead_when_spi_greater_than_one(self):
        assert interpret_spi(1.2) == "Adelantado"

    def test_behind_when_spi_less_than_one(self):
        assert interpret_spi(0.9) == "Atrasado"

    def test_on_schedule_when_spi_equals_one(self):
        assert interpret_spi(1.0) == "En cronograma"

    def test_message_when_spi_is_none(self):
        result = interpret_spi(None)
        assert result == "Sin valor planificado registrado"


class TestCalculateActivityIndicators:
    def test_calculates_all_indicators_correctly(self):
        result = calculate_activity_indicators(
            bac=1000.0,
            planned_percentage=50.0,
            actual_percentage=40.0,
            actual_cost=300.0,
        )
        assert result["pv"] == 500.0
        assert result["ev"] == 400.0
        assert result["cv"] == 100.0
        assert result["sv"] == -100.0
        assert result["cpi"] == pytest.approx(400.0 / 300.0, rel=1e-3)
        assert result["spi"] == pytest.approx(400.0 / 500.0, rel=1e-3)

    def test_cpi_eac_vac_are_none_when_ac_is_zero(self):
        result = calculate_activity_indicators(
            bac=1000.0,
            planned_percentage=50.0,
            actual_percentage=40.0,
            actual_cost=0.0,
        )
        assert result["cpi"] is None
        assert result["eac"] is None
        assert result["vac"] is None

    def test_spi_is_none_when_planned_percentage_is_zero(self):
        result = calculate_activity_indicators(
            bac=1000.0,
            planned_percentage=0.0,
            actual_percentage=0.0,
            actual_cost=0.0,
        )
        assert result["pv"] == 0.0
        assert result["ev"] == 0.0
        assert result["spi"] is None

    def test_all_zero_when_no_progress_and_no_cost(self):
        result = calculate_activity_indicators(
            bac=5000.0,
            planned_percentage=0.0,
            actual_percentage=0.0,
            actual_cost=0.0,
        )
        assert result["cv"] == 0.0
        assert result["sv"] == 0.0

    def test_fully_completed_activity_on_budget(self):
        result = calculate_activity_indicators(
            bac=1000.0,
            planned_percentage=100.0,
            actual_percentage=100.0,
            actual_cost=1000.0,
        )
        assert result["pv"] == 1000.0
        assert result["ev"] == 1000.0
        assert result["cpi"] == 1.0
        assert result["spi"] == 1.0
        assert result["eac"] == 1000.0
        assert result["vac"] == 0.0


class TestCalculateConsolidatedIndicators:
    def test_consolidates_multiple_activities(self):
        activities = [
            {"bac": 1000.0, "planned_percentage": 50.0, "actual_percentage": 40.0, "actual_cost": 300.0},
            {"bac": 2000.0, "planned_percentage": 60.0, "actual_percentage": 70.0, "actual_cost": 1500.0},
        ]
        result = calculate_consolidated_indicators(activities)
        assert result["pv"] == pytest.approx(500.0 + 1200.0, rel=1e-3)
        assert result["ev"] == pytest.approx(400.0 + 1400.0, rel=1e-3)

    def test_returns_none_cpi_when_total_ac_is_zero(self):
        activities = [
            {"bac": 1000.0, "planned_percentage": 50.0, "actual_percentage": 40.0, "actual_cost": 0.0},
            {"bac": 2000.0, "planned_percentage": 60.0, "actual_percentage": 50.0, "actual_cost": 0.0},
        ]
        result = calculate_consolidated_indicators(activities)
        assert result["cpi"] is None
        assert result["eac"] is None

    def test_single_activity_matches_individual_calculation(self):
        activities = [
            {"bac": 1000.0, "planned_percentage": 50.0, "actual_percentage": 40.0, "actual_cost": 300.0},
        ]
        consolidated = calculate_consolidated_indicators(activities)
        individual = calculate_activity_indicators(1000.0, 50.0, 40.0, 300.0)
        assert consolidated["pv"] == individual["pv"]
        assert consolidated["ev"] == individual["ev"]
        assert consolidated["cpi"] == individual["cpi"]
