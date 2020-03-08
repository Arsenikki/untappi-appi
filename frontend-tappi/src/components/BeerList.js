import React from 'react';
import { Table } from 'reactstrap';

const BeerList = ({ selectedBeers, selectedVenue }) => {
    if (selectedBeers && selectedVenue.length !== 0) {
        return (
            <div>
                <h1 className="text-center" id="tabelLabel" >Top {selectedBeers.length - 1} beers of {selectedVenue[0]}:</h1>
                <Table dark hover>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Brewery</th>
                            <th>Country</th>
                            <th>Style</th>
                            <th>Rating</th>
                            <th>Stronkness</th>

                        </tr>
                    </thead>
                    <tbody>
                        {selectedBeers.slice(1).map(beer =>
                            <tr key={beer.beerID}>
                                <td>{beer.beerName}</td>
                                <td>{beer.brewery}</td>
                                <td>{beer.country}</td>
                                <td>{beer.style}</td>
                                <td>{beer.rating} / 5</td>
                                <td>{beer.stronkness} %</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </div>
        )
    }
    else {
        return null
    }
}

export default BeerList