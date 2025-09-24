import { useMemo, useState } from "react";
import { Card } from "primereact/card";
import { Chart } from "primereact/chart";
import "chart.js/auto";
import type { TeamSummary } from "../Types/TeamSummary";

export default function WinsLossBar({ data }: { data: TeamSummary[] }) {
  const [mode, setMode] = useState<"won" | "lost">("won");

  const chartData = useMemo(() => {
    const labels = data.map((d) => d.teamName);
    const values = data.map((d) => (mode === "won" ? d.won : d.lost));
    return {
      labels,
      datasets: [
        {
          label: mode === "won" ? "Wins" : "Losses",
          data: values,
          borderWidth: 1,
          barThickness: 30, 
          maxBarThickness: 40, 
          categoryPercentage: 0.8, 
          barPercentage: 0.9, 
        },
      ],
    };
  }, [data, mode]);

  const options = useMemo(
    () => ({
      maintainAspectRatio: false,
      plugins: { legend: { display: true } },
      scales: { y: { beginAtZero: true, ticks: { precision: 0 } } },
    }),
    []
  );

  const header = (
    <div className="flex align-items-center justify-content-between w-full">
      <div className="text-slate-500 mt-1 ml-2">
        {mode === "won" ? "Wins per Team" : "Losses per Team"}
      </div>
      <button
        className="p-button p-component p-button-text p-1"
        onClick={(e) => {
          e.stopPropagation(); //prevent the click from creating unintended behaviour in parent component (below the card also accepts a click event)
          setMode((m) => (m === "won" ? "lost" : "won"));
        }}
        title="Toggle wins/losses"
        aria-label="Toggle wins/losses"
      >
        <i className="pi pi-refresh mr-2" />
        Toggle
      </button>
    </div>
  );

  return (
    <Card
      header={header}
      className="w-full rounded-2xl shadow-sm"
      onClick={() => setMode((m) => (m === "won" ? "lost" : "won"))}
    >
      <div style={{ height: 260 }}>
        <Chart type="bar" data={chartData} options={options} />
      </div>
      <small className="text-slate-500">Tip: click the card to toggle.</small>
    </Card>
  );
}

//Package for this is PrimeReact which was my table of pref: https://primereact.org
