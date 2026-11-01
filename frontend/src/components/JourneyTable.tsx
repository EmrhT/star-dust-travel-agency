import {
  Button,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material'
import type { JourneyListItem } from '../api/journeys'

// Renders JourneyListItem records and reports booking intent to JourneysPage.
// It formats backend UTC timestamps for the Europe/Istanbul browser view.
const istanbulDateTime = new Intl.DateTimeFormat('en-GB', {
  year: 'numeric',
  month: 'short',
  day: '2-digit',
  hour: '2-digit',
  minute: '2-digit',
  timeZone: 'Europe/Istanbul',
})

// Defines journey data and booking actions supplied by JourneysPage.
interface JourneyTableProps {
  journeys: JourneyListItem[]
  bookedJourneyIds: ReadonlySet<string>
  bookingActionsDisabled: boolean
  onBook: (journeyId: string) => void
}

// Builds a passenger-facing route label from direction and endpoint names.
function getRouteLabel(journey: JourneyListItem) {
  return journey.direction === 'Outbound'
    ? `${journey.launchStation} → ${journey.destination}`
    : `${journey.destination} → ${journey.launchStation}`
}

// Converts the API timestamp into the UTC+3 display format used by the table.
function formatDateTime(timestamp: string) {
  return istanbulDateTime.format(new Date(timestamp))
}

// Displays cached journeys and reports Book clicks to JourneysPage.
export function JourneyTable({
  journeys,
  bookedJourneyIds,
  bookingActionsDisabled,
  onBook,
}: JourneyTableProps) {
  return (
    <TableContainer sx={{ maxHeight: 520 }}>
      <Table
        aria-label="Generated journeys"
        size="small"
        stickyHeader
      >
        <TableHead>
          <TableRow>
            <TableCell>Route</TableCell>
            <TableCell>Direction</TableCell>
            <TableCell>Departure (GMT+3)</TableCell>
            <TableCell>Arrival (GMT+3)</TableCell>
            <TableCell>Spaceship</TableCell>
            <TableCell>Status</TableCell>
            <TableCell align="right">Booking</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {journeys.map((journey) => (
            <TableRow hover key={journey.id}>
              <TableCell>{getRouteLabel(journey)}</TableCell>
              <TableCell>{journey.direction}</TableCell>
              <TableCell>
                {formatDateTime(journey.departureAtUtc)}
              </TableCell>
              <TableCell>
                {formatDateTime(journey.arrivalAtUtc)}
              </TableCell>
              <TableCell>{journey.spaceship}</TableCell>
              <TableCell>
                <Chip
                  color="secondary"
                  label={journey.status}
                  size="small"
                  variant="outlined"
                />
              </TableCell>
              <TableCell align="right">
                <Button
                  disabled={
                    journey.status !== 'Scheduled' ||
                    bookedJourneyIds.has(journey.id) ||
                    bookingActionsDisabled
                  }
                  onClick={() => onBook(journey.id)}
                  size="small"
                  variant="outlined"
                >
                  {bookedJourneyIds.has(journey.id)
                    ? 'Booked'
                    : 'Book'}
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}
