import { TeamSummaryFetch } from "../Types/TeamSummaryFetch";

const API_BASE_URL = process.env.REACT_APP_API_BASE ?? "http://localhost:5258";

export async function GetTeamSummaries() {
  const params: TeamSummaryFetch = {
    teamId: null,
    startDate: null,
    endDate: null,
  };

  const url = `${API_BASE_URL}/api/TeamSummaries?teamId=${
    params.teamId ?? ""
  }&startDate=${params.startDate ?? ""}&endDate=${params.endDate ?? ""}`;

  const res = await fetch(url, { headers: { Accept: "application/json" } });

  if (!res.ok) {
    throw new Error(
      `GET /api/TeamSummaries failed: ${res.status} ${res.statusText}`
    );
  }
  return res.json();
}
