import { Card } from "primereact/card";
import type { TeamSummary } from "../Types/TeamSummary";

export default function TopSummaryCards({ data }: { data: TeamSummary[] }) {
  const teams = data.length;
  const totalGames = data.reduce((acc, x) => acc + x.played, 0);

  return (
    <Card className="w-full rounded-2xl shadow-sm">
      <div className="flex items-center justify-between">
        <div>
          <div className="text-slate-500 mb-2">Teams</div>
          <div className="text-3xl font-bold">{teams}</div>
          <div className="text-slate-500 mt-1">Total Games: {totalGames}</div>
        </div>
        <i className="pi pi-users text-3xl" />
      </div>
    </Card>
  );
}
