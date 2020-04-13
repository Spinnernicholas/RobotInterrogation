import * as React from 'react';
import { InterviewPosition } from '../interviewReducer';
import { PositionHeader } from './elements/PositionHeader';
import { Help } from './elements/Help';
import { P } from './elements/P';
import { Page } from './elements/Page';

interface IProps {
    position: InterviewPosition;
}

export const PositionSelection: React.FunctionComponent<IProps> = props => {
    return (
        <Page>
            <PositionHeader position={props.position} />
            <P>Wait for the Interviewer to confirm your respective <Help entry="positions">positions</Help>.</P>
        </Page>
    );
}