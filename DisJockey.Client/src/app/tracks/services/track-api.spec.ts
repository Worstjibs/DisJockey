import { TestBed } from '@angular/core/testing';

import { TrackApi } from './track-api';

describe('TrackApi', () => {
  let service: TrackApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TrackApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
