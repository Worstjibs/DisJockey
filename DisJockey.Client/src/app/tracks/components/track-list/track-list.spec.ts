import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackList } from './track-list';

describe('TrackList', () => {
  let component: TrackList;
  let fixture: ComponentFixture<TrackList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
