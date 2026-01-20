package model;

import java.util.List;

public class AppointmentRequest {
    public int requestId;
    public int personId;
    public List<Object> preferredDays;   
    public List<Integer> preferredDocs;
    public boolean isNew;
}
