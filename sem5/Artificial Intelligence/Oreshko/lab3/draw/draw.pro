% Copyright

implement draw
    open core, vpiDomains, vpi

class facts
    city : (string Name, pnt Position).
    conn : (pnt, pnt).
    distance : (string City1, string City2, real Distance).

class predicates
    connections : (windowHandle).
    drawCities : (windowHandle).

clauses
    city("Минск", pnt(30, 40)).
    city("Гомель", pnt(100, 120)).
    city("Брест", pnt(100, 160)).
    city("Могилев", pnt(200, 100)).
    conn(pnt(30, 40), pnt(100, 120)).
    conn(pnt(100, 120), pnt(100, 160)).
    conn(pnt(30, 40), pnt(200, 100)).
    distance("Минск", "Гомель", 90).
    distance("Гомель", "Брест", 60).
    distance("Гомель", "Могилев", 75).
    distance("Минск", "Брест", 140).
    distance("Минск", "Могилев", 165).
    distance("Брест", "Могилев", 230).
    drawCities(W) :-
        city(N, P),
        P = pnt(X1, Y1),
        X2 = X1 + 10,
        Y2 = Y1 + 10,
        drawEllipse(W, rct(X1, Y1, X2, Y2)),
        drawText(W, X1, Y1, N),
        fail.
    drawCities(_Win).
    connections(Win) :-
        conn(P1, P2),
        drawLine(Win, P1, P2),
        fail.
    connections(_Win).
    drawThem(Win) :-
        connections(Win),
        drawCities(Win).
    findDistance(City1, City2, EditCtrl) :-
        EditCtrl:setText("No info"),
        (distance(City1, City2, Distance) or distance(City2, City1, Distance)),
        EditCtrl:setText(toString(Distance)),
        fail.
    findDistance(_City1, _City2, _EditCtrl).

end implement draw
