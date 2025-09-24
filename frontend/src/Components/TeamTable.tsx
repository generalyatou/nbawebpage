import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Tag } from 'primereact/tag';
import type { TeamSummary } from '../Types/TeamSummary';
import { getTeamLogo } from '../utils/teamLogoMap';

export default function TeamTable({ data, loading }: { data: TeamSummary[]; loading?: boolean }) {
  //Last minute decision due to LOGO/URL length restraints. Explain on call
  const logoBody = (row: TeamSummary) => (
    <div className="flex align-items-center gap-2">
      <img
        src={getTeamLogo(row.teamName)}
        alt={row.teamName}
        width={28}
        height={28}
        style={{ objectFit: 'cover', borderRadius: 6 }}
      />
      <span>{row.teamName}</span>
    </div>
  );

const numberBody = (val: number) => <span className="font-medium">{val}</span>;

  return (
    <DataTable value={data} loading={loading} paginator rows={8} stripedRows showGridlines
      className="p-datatable-sm border-round-2xl shadow-2"
      sortMode="multiple" removableSort emptyMessage="No team summaries">
      <Column header="Team" body={logoBody} sortable />
      <Column field="stadium" header="Stadium" sortable />
      <Column field="mvp" header="MVP" sortable />
      <Column field="played" header="Played" body={(r) => numberBody(r.played)} sortable />
      <Column field="won" header="Won" body={(r) => <Tag value={r.won} severity="success" />} sortable />
      <Column field="lost" header="Lost" body={(r) => <Tag value={r.lost} severity="danger" />} sortable />
      <Column field="playedHome" header="Home" body={(r) => numberBody(r.playedHome)} sortable />
      <Column field="playedAway" header="Away" body={(r) => numberBody(r.playedAway)} sortable />
      <Column field="biggestWin" header="Biggest Win" />
      <Column field="biggestLoss" header="Biggest Loss" />
      <Column field="lastGameStadium" header="Last Game Stadium" />
      <Column field="lastGameDate" header="Last Game" sortable
        body={(r) => new Date(r.lastGameDate).toLocaleDateString()} />
    </DataTable>
  );
}
