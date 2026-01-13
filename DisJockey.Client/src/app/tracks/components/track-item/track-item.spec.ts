import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackItem } from './track-item';

describe('TrackItem', () => {
  let component: TrackItem;
  let fixture: ComponentFixture<TrackItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
