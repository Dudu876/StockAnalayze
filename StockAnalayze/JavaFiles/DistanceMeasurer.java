package solution;


public class DistanceMeasurer {

	public static final double measureDistance(ClusterCenter center, Vector v) {
		double sum = 0;
		int length = v.getVector().length;
		for (int i = 0; i < length; i++) {
			sum += Math.abs(center.getCenter().getVector()[i] - v.getVector()[i]);
		}

		return sum;
	}
	
	public static final double measureDistance(Vector center, Vector v) {
		double sum = 0;
		int length = v.getVector().length;
		for (int i = 0; i < length; i++) {
			sum += Math.pow((center.getVector()[i] - v.getVector()[i]),2);
		}

		return Math.sqrt(sum);
	}
}
