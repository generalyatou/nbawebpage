import { useEffect, useState } from "react";
import type { TeamSummary } from "./Types/TeamSummary";
import { Card } from "primereact/card";
import { GetTeamSummaries } from "./Services/api";
import TopSummaryCards from "./Components/TopSummaryCards";
import WinsLossBar from "./Components/WinLossBar";
import TeamTable from "./Components/TeamTable";

export default function App() {
  const [data, setData] = useState<TeamSummary[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const rows = await GetTeamSummaries();
        setData(rows);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  return (
    <div className="mx-auto max-w-6xl p-4 lg:p-6">
      <header className="mb-4 flex items-center justify-between">
        <h2 className="m-0 text-xl font-semibold">NBA Team Summaries</h2>
      </header>

      <div className="rounded-2xl border border-slate-200 bg-gradient-to-r from-sky-50 to-indigo-50/50 p-3 mb-4">
        <div className="flex flex-col md:flex-row items-start gap-4">
          <div className="w-full md:w-1/3">
            <TopSummaryCards data={data} />
          </div>
          <div className="w-full md:w-2/3">
            <WinsLossBar data={data} />
          </div>
        </div>
      </div>

      <section className="mt-4">
        <Card className="shadow-2 border-round-2xl">
          <h3 className="mt-0">Teams</h3>
          <TeamTable data={data} loading={loading} />
        </Card>
      </section>
    </div>
  );
}
