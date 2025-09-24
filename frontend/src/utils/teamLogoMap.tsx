import bulls from '../assets/logos/bulls.png';
import nuggets from '../assets/logos/nuggets.png';
import rockets from '../assets/logos/rockets.png';
import pacers from '../assets/logos/pacers.png';
import heat from '../assets/logos/heat.png';
import thunder from '../assets/logos/thunder.png';
import spurs from '../assets/logos/spurs.png';
import raptors from '../assets/logos/raptors.png';
import generic from '../assets/logos/generic.png';

function keyOf(name: string) {
  return name?.trim().toLowerCase().replace(/\s+/g, ' ') ?? '';
}

const MAP: Record<string, string> = {
  [keyOf('Chicago Bulls')]: bulls,
  [keyOf('Denver Nuggets')]: nuggets,
  [keyOf('Houston Rockets')]: rockets,
  [keyOf('DHouston Rockets')]: rockets, 
  [keyOf('Indiana Pacers')]: pacers,
  [keyOf('Miami Heat')]: heat,
  [keyOf('Oklahoma City Thunder')]: thunder,
  [keyOf('San Antonio Spurs')]: spurs,
  [keyOf('Toronto Raptors')]: raptors,
};

export function getTeamLogo(teamName: string | undefined | null): string {
  const k = keyOf(teamName ?? '');
  return MAP[k] ?? generic;
}
