from typing import Optional


def calculate_pv(bac: float, planned_percentage: float) -> float:
    return round((planned_percentage / 100.0) * bac, 4)


def calculate_ev(bac: float, actual_percentage: float) -> float:
    return round((actual_percentage / 100.0) * bac, 4)


def calculate_cv(ev: float, ac: float) -> float:
    return round(ev - ac, 4)


def calculate_sv(ev: float, pv: float) -> float:
    return round(ev - pv, 4)


def calculate_cpi(ev: float, ac: float) -> Optional[float]:
    if ac == 0:
        return None
    return round(ev / ac, 4)


def calculate_spi(ev: float, pv: float) -> Optional[float]:
    if pv == 0:
        return None
    return round(ev / pv, 4)


def calculate_eac(bac: float, cpi: Optional[float]) -> Optional[float]:
    if cpi is None or cpi == 0:
        return None
    return round(bac / cpi, 4)


def calculate_vac(bac: float, eac: Optional[float]) -> Optional[float]:
    if eac is None:
        return None
    return round(bac - eac, 4)


def interpret_cpi(cpi: Optional[float]) -> str:
    if cpi is None:
        return "Sin costo real registrado"
    if cpi > 1:
        return "Bajo presupuesto"
    if cpi < 1:
        return "Sobre presupuesto"
    return "En presupuesto"


def interpret_spi(spi: Optional[float]) -> str:
    if spi is None:
        return "Sin valor planificado registrado"
    if spi > 1:
        return "Adelantado"
    if spi < 1:
        return "Atrasado"
    return "En cronograma"


def calculate_activity_indicators(
    bac: float,
    planned_percentage: float,
    actual_percentage: float,
    actual_cost: float,
) -> dict:
    pv = calculate_pv(bac, planned_percentage)
    ev = calculate_ev(bac, actual_percentage)
    cpi = calculate_cpi(ev, actual_cost)
    spi = calculate_spi(ev, pv)
    eac = calculate_eac(bac, cpi)

    return {
        "pv": pv,
        "ev": ev,
        "cv": calculate_cv(ev, actual_cost),
        "sv": calculate_sv(ev, pv),
        "cpi": cpi,
        "spi": spi,
        "eac": eac,
        "vac": calculate_vac(bac, eac),
        "cpi_interpretation": interpret_cpi(cpi),
        "spi_interpretation": interpret_spi(spi),
    }


def calculate_consolidated_indicators(activities_data: list[dict]) -> dict:
    total_bac = sum(a["bac"] for a in activities_data)
    total_pv = sum(calculate_pv(a["bac"], a["planned_percentage"]) for a in activities_data)
    total_ev = sum(calculate_ev(a["bac"], a["actual_percentage"]) for a in activities_data)
    total_ac = sum(a["actual_cost"] for a in activities_data)

    cpi = calculate_cpi(total_ev, total_ac)
    spi = calculate_spi(total_ev, total_pv)
    eac = calculate_eac(total_bac, cpi)

    return {
        "pv": round(total_pv, 4),
        "ev": round(total_ev, 4),
        "cv": calculate_cv(total_ev, total_ac),
        "sv": calculate_sv(total_ev, total_pv),
        "cpi": cpi,
        "spi": spi,
        "eac": eac,
        "vac": calculate_vac(total_bac, eac),
        "cpi_interpretation": interpret_cpi(cpi),
        "spi_interpretation": interpret_spi(spi),
    }
