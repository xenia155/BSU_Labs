% Copyright

namespace collectionSupport

implement map inherits formWindow
    open core, vpiDomains

clauses
    display(Parent) = Form :-
        Form = new(Parent),
        Form:show().

clauses
    new(Parent) :-
        formWindow::new(Parent),
        generatedInitialize().

predicates
    onPaint : window::paintResponder.
clauses
    onPaint(_Source, _Rectangle, _GDI) :-
        W = _Source:getVPIWindow(),
        draw::drawThem(W).

predicates
    onPushButtonClick : button::clickResponder.
clauses
    onPushButtonClick(_Source) = button::defaultAction :-
        City1 = edit_ctl:getText(),
        City2 = edit1_ctl:getText(),
        Distance = draw::findDistance(City1, City2),
        DistanceStr = toString(Distance),
        staticText2_ctl:setText(DistanceStr).

% This code is maintained automatically, do not update it manually.
facts
    ok_ctl : button.
    cancel_ctl : button.
    help_ctl : button.
    edit_ctl : editControl.
    edit1_ctl : editControl.
    pushButton_ctl : button.
    staticText2_ctl : textControl.

predicates
    generatedInitialize : ().
clauses
    generatedInitialize() :-
        setText("map"),
        setRect(vpiDomains::rct(50, 40, 390, 288)),
        setDecoration(titlebar([frameDecoration::maximizeButton, frameDecoration::minimizeButton, frameDecoration::closeButton])),
        setBorder(frameDecoration::sizeBorder),
        setState([wsf_ClipSiblings, wsf_ClipChildren]),
        menuSet(noMenu),
        setPaintResponder(onPaint),
        ok_ctl := button::newOk(This),
        ok_ctl:setText("&OK"),
        ok_ctl:setPosition(20, 212),
        ok_ctl:setSize(56, 16),
        ok_ctl:defaultHeight := false,
        ok_ctl:setAnchors([control::right, control::bottom]),
        cancel_ctl := button::newCancel(This),
        cancel_ctl:setText("Cancel"),
        cancel_ctl:setPosition(136, 212),
        cancel_ctl:setSize(56, 16),
        cancel_ctl:defaultHeight := false,
        cancel_ctl:setAnchors([control::right, control::bottom]),
        help_ctl := button::new(This),
        help_ctl:setText("&Help"),
        help_ctl:setPosition(248, 212),
        help_ctl:setSize(56, 16),
        help_ctl:defaultHeight := false,
        help_ctl:setAnchors([control::right, control::bottom]),
        StaticText_ctl = textControl::new(This),
        StaticText_ctl:setText("Город 1:"),
        StaticText_ctl:setPosition(220, 148),
        StaticText_ctl:setWidth(36),
        StaticText1_ctl = textControl::new(This),
        StaticText1_ctl:setText("Город 2:"),
        StaticText1_ctl:setPosition(220, 166),
        StaticText1_ctl:setWidth(36),
        edit_ctl := editControl::new(This),
        edit_ctl:setPosition(276, 148),
        edit1_ctl := editControl::new(This),
        edit1_ctl:setPosition(276, 166),
        pushButton_ctl := button::new(This),
        pushButton_ctl:setText("Найти расстояние:"),
        pushButton_ctl:setPosition(216, 184),
        pushButton_ctl:setWidth(68),
        pushButton_ctl:setClickResponder(onPushButtonClick),
        staticText2_ctl := textControl::new(This),
        staticText2_ctl:setPosition(284, 188),
        staticText2_ctl:setWidth(40).
% end of automatic code

end implement map
