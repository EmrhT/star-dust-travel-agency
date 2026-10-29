import {
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material'
import type { BookingListItem } from '../api/bookings'

// Renders App's identity-filtered BookingListItem records in a table.
// Backend UTC timestamps are formatted for the Europe/Istanbul browser view.
const istanbulDateTime = new Intl.DateTimeFormat('en-GB', {
  year: 'numeric',
  month: 'short',
  day: '2-digit',
  hour: '2-digit',
  minute: '2-digit',
  timeZone: 'Europe/Istanbul',
})

// Defines the owned booking collection passed from App to BookingTable.
interface BookingTableProps {
  bookings: BookingListItem[]
}

// Converts an API UTC timestamp for the booking table's GMT+3 columns.
function formatDateTime(timestamp: string) {
  return istanbulDateTime.format(new Date(timestamp))
}

// Displays owned bookings without fetching or changing server state.
export function BookingTable({ bookings }: BookingTableProps) {
  return (
    <TableContainer sx={{ maxHeight: 420 }}>
      <Table aria-label="My bookings" size="small" stickyHeader>
        <TableHead>
          <TableRow>
            <TableCell>Launch station</TableCell>
            <TableCell>Destination</TableCell>
            <TableCell>Departure (GMT+3)</TableCell>
            <TableCell>Spaceship</TableCell>
            <TableCell>Booked at (GMT+3)</TableCell>
            <TableCell>Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {bookings.map((booking) => (
            <TableRow hover key={booking.id}>
              <TableCell>{booking.launchStation}</TableCell>
              <TableCell>{booking.destination}</TableCell>
              <TableCell>
                {formatDateTime(booking.departureAtUtc)}
              </TableCell>
              <TableCell>{booking.spaceship}</TableCell>
              <TableCell>{formatDateTime(booking.bookedAtUtc)}</TableCell>
              <TableCell>
                <Chip
                  color="secondary"
                  label={booking.status}
                  size="small"
                  variant="outlined"
                />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}
